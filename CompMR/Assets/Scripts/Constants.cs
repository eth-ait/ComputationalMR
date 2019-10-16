namespace Assets.Scripts
{
    public class Constants
    {
        public static float INITIAL_VISIBILITY_REWARD = .2f;
        public static float INITIAL_COGNITIVE_CAPACITY = .35f;

        public static int MIN_PLACEMENT_SLOTS = 1;
        public static int TOTAL_NUM_VIEW_SLOTS = 4;

        public static bool ENABLE_DEBUG_OUTPUT = true;

        public static float INITIAL_WEIGHT_IMPORTANCE = .5f;

        public const int MAX_IMPORTANCE = 7;
        public const int MAX_COGNITIVE_LOAD = 7;


        public static float INITIAL_COGNITIVE_LOAD_ONSET_PENALTY = .0f;
        public static float INITIAL_COGNITIVE_LOAD_ONSET_PENALTY_DECAY_TIMESTEPS = 5;
        public static float INITIAL_ELEMENT_MAX_COG_LOAD_OFFSET = .2f;

        public static float INITIAL_MIN_CHANGE_COGN_CAPACITY = .1f;
        public static float SLOT_SIDE_LENGTH = .3f;
        public static float INITIAL_OCCLUSION_OFFSET = .2f;
        public static float INITIAL_MIN_PLACEMENT_DISTANCE = .3f;

        public static float INITIAL_SLOTS_HEIGHT_MIN = .8f;
        public static float INITIAL_SLOTS_HEIGHT_MAX = 2f;

        public static float MAX_SLOT_ANGLE = 45.0f;

        public static int INITIAL_NUM_BEST_SLOTS_DISPLAY = 10;

        public static int INITIAL_QUALITY_STEPS_INTEGRATE = 300;
        public static bool ENABLE_QUALITY_INTEGRATION = true;

        public static float INITIAL_WEIGHT_DISTANCE = .3f;

        public static float INITIAL_VIEW_SLOT_SIDE_LENGTH = .3f;
        public static float INITIAL_SLOTS_DISTANCE_TO_HMD = .8f;

        public static float IPA_UPPER_BOUND = .5f;
        public static float IPA_LOWER_BOUND = .05f;

        public static int NUM_ICONS_IN_LIBRARY = 200;

        public static float ANIMATION_STEPS = 90.0f;

        public static float VIEW_SCALE_FACTOR = .33f;

        public static float TEMP_FIXATE_SEC = 5;
    }
}